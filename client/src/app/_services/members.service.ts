import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) {         // injecting services into services is fine, unless you create a service dependency cycle with this
    // This was before I introduced "clearing of the cache" after loging out:
    // this.accountService.currentUser$.pipe(take(1)).subscribe({
    // Now it is:
    this.accountService.currentUser$.subscribe({        // track the changes of currentUser$
      next: user => {
        if (user) {         // when user logs in
          this.userParams = new UserParams(user);
          this.user = user;
        }
        // else statement was not present until "clearing of the cache" after loging out was introduced:
        else {              // when user logs out
          this.clearMemberCacheOnLogOut();
        }
      }
    })
  }

  clearMemberCacheOnLogOut() {
    this.members = [];
    this.memberCache = new Map();
    this.user = undefined;
    this.userParams = undefined;
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;   // if not present, then you have a warning (not a red error, but yellow warning)
  }

  getMembers(userParams: UserParams) {
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(      // the only reason we have here .pipe() and then .map() is because we want to store the response in cache, just because of that
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])             // https://www.google.com/search?q=reduce+function+typescript (or javascript), e.g.: https://stackoverflow.com/questions/14087489/typescript-and-array-reduce-function
      .find((member: Member) => member.userName === username);        // returns the FIRST (!) element that matches the condition
      // memberCache may have duplicates (or triplicates, etc.) of the same user, but this is not as bad as calling an API method each time, and at the same time it is not worth finding these duplicates every time (in this particular example)

    if (member) return of(member);
    
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member}
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});         // the last param (body) is empty, we do not pass anything except for the photoId param in the url itself
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    // this was before:
    // return this.http.get<Member[]>(this.baseUrl + 'likes?predicate=' + predicate);      // here we can allow ourselves to put "predicate" variable explicitly since we have only one variable in the request
    // now it is:
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }

}
