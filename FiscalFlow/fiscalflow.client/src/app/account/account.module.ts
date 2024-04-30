import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { AddAccountComponent } from './add-account/add-account.component';
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";
import { AccountComponent } from './account/account.component';
import {MatCardModule} from "@angular/material/card";


@NgModule({
  declarations: [
    AddAccountComponent,
    AccountComponent
  ],
  exports: [
    AccountComponent
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    SharedModule,
    FormsModule,
    MatCardModule
  ]
})
export class AccountModule { }
