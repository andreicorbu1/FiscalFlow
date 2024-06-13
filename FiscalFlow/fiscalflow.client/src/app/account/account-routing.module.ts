import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddAccountComponent } from './add-account/add-account.component';
import { AccountDetailComponent } from './account-detail/account-detail.component';
import { DashboardComponent } from '../dashboard/dashboard.component';

const routes: Routes = [
  { path: 'create-account', component: AddAccountComponent },
  { path: 'edit/:id', component: AddAccountComponent },
  { path: 'details/:id', component: AccountDetailComponent },
  { path: 'dashboard', component: DashboardComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
