import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { ResetPassword } from 'src/app/shared/models/account/resetPassword';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup = new FormGroup({});
  token: string | undefined;
  email: string | undefined;
  submitted: boolean = false;
  errorMessages: string[] = [];
  constructor(
    private accountService: AccountService,
    private sharedService: SharedService,
    private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              this.token = params.get('token');
              this.email = params.get('email');
              if (this.token && this.email) {
                this.initializeForm(this.email);
              } else {
                this.router.navigateByUrl('/account/login');
              }
            },
          });
        }
      },
    });
  }

  initializeForm(username: string) {
    this.resetPasswordForm = this.formBuilder.group({
      email: [{ value: username, disabled: true }],
      newPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(128),
        ],
      ],
    });
  }

  resetPassword() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.resetPasswordForm.valid && this.email && this.token) {
      const model: ResetPassword = {
        email: this.email,
        resetCode: this.token,
        newPassword: this.resetPasswordForm.get('newPassword')?.value,
      };
      this.accountService.resetPassword(model).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(
            true,
            response.title,
            response.message
          );
          this.router.navigateByUrl('/account/login');
        },
        error: (error: any) => {
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
}
