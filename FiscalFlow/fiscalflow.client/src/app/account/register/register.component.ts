import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit{
  
  registerForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private router: Router) { }

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
      this.accountService.register(this.registerForm.value).subscribe({
        next: (response) => {
          this.sharedService.showNotification(true, "Account created", "Your account has been created successfully. Please login.");
          this.router.navigateByUrl('/account/login');
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
