import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { AddAccountComponent } from './add-account/add-account.component';
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";
import { AccountComponent } from './account/account.component';
import {MatCardModule} from "@angular/material/card";
import {MatButtonModule} from "@angular/material/button";
import {MatTooltipModule} from "@angular/material/tooltip";
import {MatIconModule} from "@angular/material/icon";
import { AccountDetailComponent } from './account-detail/account-detail.component';

@NgModule({
  declarations: [
    AddAccountComponent,
    AccountComponent,
    AccountDetailComponent
  ],
  exports: [
    AccountComponent
  ],
    imports: [
        CommonModule,
        AccountRoutingModule,
        SharedModule,
        MatIconModule,
        FormsModule,
        MatCardModule,
        MatButtonModule,
        MatTooltipModule
    ]
})
export class AccountModule { }
