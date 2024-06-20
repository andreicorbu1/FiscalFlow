import { Component, NgZone, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/shared/models/user/user';
import { take } from 'rxjs';
import { ExternalAuth } from 'src/app/shared/models/user/externalAuth';
import {
  FacebookLoginProvider,
  SocialAuthService,
} from '@abacritt/angularx-social-login';
import { MatIcon } from '@angular/material/icon';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];
  returnUrl: string | null = null;

  constructor(
    private authService: SocialAuthService,
    private userService: UserService,
    private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private ngZone: NgZone
  ) {
    this.userService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              if (params) {
                this.returnUrl = params.get('returnUrl');
              }
            },
          });
        }
      },
    });
  }

  async handleCredentialResponse(response: any) {
    this.ngZone.run(() => {
      const externalAuth: ExternalAuth = {
        provider: 'Google',
        idToken: response.credential,
      };
      this.userService.externalLogin(externalAuth);
    });
  }

  signInWithFB(): void {
    this.authService.signIn(FacebookLoginProvider.PROVIDER_ID).then((value) => {
      console.log(value);
      const externalAuth: ExternalAuth = {
        provider: 'Facebook',
        idToken: value.response.id,
        //@ts-ignore
        AccessToken: value.response.authToken,
        Email: value.email,
        FirstName: value.firstName,
        LastName: value.lastName,
      };
      this.userService.externalLogin(externalAuth);
    });
  }

  ngOnInit(): void {
    // @ts-ignore
    google.accounts.id.initialize({
      client_id:
        '569967023098-ot4j9ac9t0u6mebpff563115f8qm5ore.apps.googleusercontent.com',
      callback: this.handleCredentialResponse.bind(this),
      auto_select: false,
      cancel_on_tap_outside: true,
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      document.getElementById('google-button'),
      { theme: 'outline', size: 'large', shape: 'pill', width: 250 }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => {});
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      email: [
        '',
        [
          Validators.pattern(
            "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"
          ),
          Validators.required,
        ],
      ],
      password: ['', Validators.required],
    });
  }

  login() {
    this.errorMessages = [];
    this.submitted = true;
    if (this.loginForm.valid) {
      this.userService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          if (this.returnUrl) {
            this.router.navigateByUrl(this.returnUrl);
          } else {
            this.router.navigateByUrl('/');
          }
        },
        error: (error) => {
          if (error.error.errors) {
            this.errorMessages = error.error.errors;
          } else if (Array.isArray(error.error)) {
            error.error.forEach(
              (element: { code: string; description: string }) => {
                this.errorMessages.push(element.description);
              }
            );
          } else {
            this.errorMessages.push(error.error);
          }
        },
      });
    }
  }
  resendEmailConfirmationLink() {}
}
