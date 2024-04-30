import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {AddAccountComponent} from "./add-account/add-account.component";

const routes: Routes = [
  { path: 'create-account', component: AddAccountComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
