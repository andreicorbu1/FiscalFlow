import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { SendEmailComponent } from './send-email/send-email.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { GoogleSigninButtonModule } from '@abacritt/angularx-social-login';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ConfirmEmailComponent,
    SendEmailComponent,
    ResetPasswordComponent
  ],
  imports: [
    CommonModule,
    GoogleSigninButtonModule,
    AccountRoutingModule,
    SharedModule
  ]
})
export class AccountModule { }
