import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize, identity } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy();
    
    return next.handle(request).pipe(
      (environment.production ? identity : delay(1000)),        // if not in production mode, return an artificial delay of 1 sec
      finalize(() => {                                          // 'identity' function is the same as 'x => x' (that is, we just return the input parameter without any changes, which is our case since we don't want to introduce any artificial delays in case of production mode)
        this.busyService.idle()
      })
    )
  }
}
