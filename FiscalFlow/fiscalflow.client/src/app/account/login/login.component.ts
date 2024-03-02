import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit{
  loginForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private formBuilder: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.pattern("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"), Validators.required]],
      password: ['', Validators.required],
    });
  }

  login() {
    this.errorMessages = [];
    this.submitted = true;
    if(this.loginForm.valid) {
      this.accountService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          this.router.navigateByUrl('/');
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
