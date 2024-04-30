import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/user/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit{

  registerForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];

  constructor(private userService: UserService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private router: Router) {
      this.userService.user$.pipe(take(1)).subscribe({
        next: (user: User | null) => {
          if(user) {
            this.router.navigateByUrl('/');
          }
        }
      });
    }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
      lastName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
      email: ['', [Validators.pattern("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"), Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(128)]],
    });
  }

  register() {
    this.errorMessages = [];
    this.submitted = true;
    if(this.registerForm.valid) {
      this.userService.register(this.registerForm.value).subscribe({
        next: (response) => {
          this.sharedService.showNotification(true, "Account created", "Your user has been created successfully. Please confirm your email address.");
          this.router.navigateByUrl('/user/login');
        },
        error: error => {
          if(error.error.errors) {
            this.errorMessages = error.error.errors;
          }
          else if (Array.isArray(error.error)) {
            error.error.forEach((element: { code: string; description: string; }) => {
              this.errorMessages.push(element.description);
            });
          } else {
            this.errorMessages.push(error.error);
          }
        }
      });
    }
  }
}
