import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import {Observable, catchError, take, throwError} from 'rxjs';
import {UserService} from 'src/app/user/user.service';
import {environment} from "../../../environments/environment.development";
import {User} from "../models/user/user";
import { SharedService } from '../shared.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private sharedService: SharedService, private userService: UserService){}
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let user: User = JSON.parse(localStorage.getItem(environment.userKey) || '""');
    if(request.url.includes('openai')) {
      return next.handle(request);
    }
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    });
    return next.handle(request).pipe(catchError((error: HttpErrorResponse) => {
      if(error.status === 401 && user.token !== undefined) {
        this.sharedService.showNotification(false, "Error", "Session expired. Please login again.");
        this.userService.logout();
      }
      return throwError(error);
    }));
  }
}
