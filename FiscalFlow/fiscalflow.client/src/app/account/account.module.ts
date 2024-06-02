import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountRoutingModule } from './account-routing.module';
import { AddAccountComponent } from './add-account/add-account.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { AccountComponent } from './account/account.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { AccountDetailComponent } from './account-detail/account-detail.component';
import { TransactionsTabelComponent } from '../transaction/transactions-tabel/transactions-tabel.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { CategorySpendingComponent } from './category-spending/category-spending.component';
import { MatGridListModule } from '@angular/material/grid-list';
import { CategoryPipePipe } from './category-pipe.pipe';
import { MatMenuModule } from '@angular/material/menu';
import { NgChartsModule } from 'ng2-charts';
@NgModule({
  declarations: [
    AddAccountComponent,
    AccountComponent,
    AccountDetailComponent,
    CategorySpendingComponent,
    CategoryPipePipe,
  ],
  exports: [AccountComponent, CategorySpendingComponent],
  imports: [
    CommonModule,
    AccountRoutingModule,
    SharedModule,
    MatIconModule,
    NgChartsModule,
    FormsModule,
    MatFormFieldModule,
    MatCardModule,
    MatButtonModule,
    MatMenuModule,
    MatTooltipModule,
    TransactionsTabelComponent,
    MatDialogModule,
    MatInputModule,
    MatSelectModule,
    MatRadioModule,
    MatDatepickerModule,
    MatGridListModule,
  ],
})
export class AccountModule {}
