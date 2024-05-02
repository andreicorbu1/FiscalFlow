import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {Observable, take} from 'rxjs';
import {UserService} from 'src/app/user/user.service';
import {environment} from "../../../environments/environment.development";
import {User} from "../models/user/user";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let user: User = JSON.parse(localStorage.getItem(environment.userKey) || '""');
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    });
    return next.handle(request);
  }
}
