import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { SharedService } from 'src/app/shared/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/user/user';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss'],
})
export class SendEmailComponent implements OnInit {
  emailForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  mode: string | undefined;
  errorMessages: string[] = [];
  constructor(
    private userService: UserService,
    private sharedService: SharedService,
    private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.userService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        } else {
          const mode = this.activatedRoute.snapshot.paramMap.get('mode');
          if (mode) {
            this.mode = mode;
            console.log(this.mode);
            this.initializeForm();
          }
        }
      },
    });
  }

  initializeForm() {
    this.emailForm = this.formBuilder.group({
      email: [
        '',
        [
          Validators.pattern(
            "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"
          ),
          Validators.required,
        ],
      ],
    });
  }

  sendEmail() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.emailForm.valid && this.mode) {
      if (this.mode.includes('resend-email-confirmation-link')) {
        this.userService
          .resendEmailConfirmationLink(this.emailForm.get('email')?.value)
          .subscribe({
            next: (response: any) => {
              this.sharedService.showNotification(
                true,
                response.title,
                response.message
              );
              this.router.navigateByUrl('/user/login');
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
      } else if (this.mode.includes('forgot-password')) {
        this.userService
          .forgotPassword(this.emailForm.get('email')?.value)
          .subscribe({
            next: (response: any) => {
              this.sharedService.showNotification(
                true,
                response.title,
                response.message
              );
              this.router.navigateByUrl('/user/login');
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
  }
  cancel() {
    this.router.navigateByUrl('/user/login');
  }
}
