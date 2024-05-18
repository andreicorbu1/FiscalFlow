import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserRoutingModule } from './user-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { SendEmailComponent } from './send-email/send-email.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { MatButton, MatButtonModule } from '@angular/material/button';
// import { GoogleSigninButtonModule } from '@abacritt/angularx-social-login';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ConfirmEmailComponent,
    SendEmailComponent,
    ResetPasswordComponent,
  ],
  imports: [CommonModule, MatButtonModule, UserRoutingModule, SharedModule],
})
export class UserModule {}
