import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/models/user/register';
import { environment } from 'src/environments/environment.development';
import { Login } from '../shared/models/user/login';
import { User } from '../shared/models/user/user';
import { ReplaySubject, map, of } from 'rxjs';
import { Router } from '@angular/router';
import { ConfirmEmail } from '../shared/models/user/confirmEmail';
import { ResetPassword } from '../shared/models/user/resetPassword';
import { ExternalAuth } from '../shared/models/user/externalAuth';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userSource = new ReplaySubject<User | null>(1);

  user$ = this.userSource.asObservable();

  constructor(private httpClient: HttpClient, private router: Router) {}

  externalLogin(externalAuth: ExternalAuth) {
    this.httpClient
      .post<User>(
        `${environment.appUrl}/api/v1/user/external-login`,
        externalAuth
      )
      .subscribe({
        next: (user: User) => {
          if (user) {
            this.setUser(user);
            this.router.navigateByUrl('/');
          }
        },
        error: (error) => {
          console.log(error);
        },
      });
  }

  login(model: Login) {
    return this.httpClient
      .post<User>(`${environment.appUrl}/api/v1/user/login`, model)
      .pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          }
        })
      );
  }

  refreshUser(jwt: string | null) {
    if (jwt === null) {
      this.userSource.next(null);
      return of(undefined);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${jwt}`);

    return this.httpClient
      .get<User>(`${environment.appUrl}/api/v1/user/refresh-user-token`, {
        headers,
      })
      .pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          }
        })
      );
  }

  register(model: Register) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/user/register`,
      model
    );
  }

  logout() {
    this.httpClient
      .put(`${environment.appUrl}/api/v1/user/revoke-refresh-token`, {})
      .subscribe(() => {
        localStorage.removeItem(environment.userKey);
        this.userSource.next(null);
        this.router.navigateByUrl('/');
      });
  }

  getJwt() {
    const key = localStorage.getItem(environment.userKey);
    if (key) {
      const user: User = JSON.parse(key);
      return user;
    }
    return null;
  }

  confirmEmail(model: ConfirmEmail) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/user/confirm-email`,
      model
    );
  }

  setUser(user: User) {
    localStorage.setItem(environment.userKey, JSON.stringify(user));
    this.userSource.next(user);
  }

  forgotPassword(email: string) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/user/forgot-password/${email}`,
      {}
    );
  }

  resendEmailConfirmationLink(email: string) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/user/resend-email-confirmation-link/${email}`,
      {}
    );
  }

  resetPassword(model: ResetPassword) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/user/reset-password`,
      model
    );
  }
}
