import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/models/account/register';
import { environment } from 'src/environments/environment.development';
import { Login } from '../shared/models/account/login';
import { User } from '../shared/models/account/user';
import { ReplaySubject, map, of } from 'rxjs';
import { Router } from '@angular/router';
import { ConfirmEmail } from '../shared/models/account/confirmEmail';
import { ResetPassword } from '../shared/models/account/resetPassword';
import { ExternalAuth } from '../shared/models/account/externalAuth';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private userSource = new ReplaySubject<User | null>(1);

  user$ = this.userSource.asObservable();

  constructor(private httpClient: HttpClient, private router: Router) {}

  externalLogin(externalAuth: ExternalAuth) {
    console.log('ajung aici?');
    this.httpClient
      .post<User>(
        `${environment.appUrl}/api/v1/account/external-login`,
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
      .post<User>(`${environment.appUrl}/api/v1/account/login`, model)
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
      .get<User>(`${environment.appUrl}/api/v1/account/refresh-user-token`, {
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
      `${environment.appUrl}/api/v1/account/register`,
      model
    );
  }

  logout() {
    localStorage.removeItem(environment.userKey);
    this.userSource.next(null);
    this.router.navigateByUrl('/');
  }

  getJwt() {
    const key = localStorage.getItem(environment.userKey);
    if (key) {
      const user: User = JSON.parse(key);
      return user.jwt;
    }
    return null;
  }

  checkAuthorized() {
    this.httpClient
      .get(`${environment.appUrl}/api/v1/account/check-authorized`)
      .subscribe((response) => console.log(response));
  }

  confirmEmail(model: ConfirmEmail) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/account/confirm-email`,
      model
    );
  }

  private setUser(user: User) {
    localStorage.setItem(environment.userKey, JSON.stringify(user));
    this.userSource.next(user);
  }

  forgotPassword(email: string) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/account/forgot-password/${email}`,
      {}
    );
  }

  resendEmailConfirmationLink(email: string) {
    return this.httpClient.post(
      `${environment.appUrl}/api/v1/account/resend-email-confirmation-link/${email}`,
      {}
    );
  }

  resetPassword(model: ResetPassword) {
    return this.httpClient.put(
      `${environment.appUrl}/api/v1/account/reset-password`,
      model
    );
  }
}
