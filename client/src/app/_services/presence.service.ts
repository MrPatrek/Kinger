import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {     // username var is "newUserLoggedIn"
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUsersSource.next([...usernames, username])    // usernames var is "storedUsernamesUpToNow"
      })
    })
    
    this.hubConnection.on('UserIsOffline', username => {    // username var is "userLoggedOut"
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))       // usernames var is "storedUsernamesUpToNow"
      })
    })

    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + ' has send you a new message! Click me to see it')
        .onTap
        .pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')         // if in some other chat already and if clicked, then customRouteReuseStrategy will "clear" current Messages tab and put the correct Messages (for the user that you clicked (the notification)). If not customRouteReuseStrategy and still in char with other user (different from the one in notification), then, after clicking the notification, old Messages tab will preserve, even though we clicked a notification which redicrects to new chat. But customRouteReuseStrategy solves this problem.
        })
    })
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
