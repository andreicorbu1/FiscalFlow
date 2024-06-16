import { Component, Inject, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Transaction } from 'src/app/shared/models/transaction/transaction';
import { CategoryPipePipe } from 'src/app/account/category-pipe.pipe';
interface Category {
  value: number;
  viewValue: string;
}

@Component({
  selector: 'app-transaction-details',
  templateUrl: './transaction-details.component.html',
  styleUrls: ['./transaction-details.component.scss'],
})
export class TransactionDetailsComponent {
  markerPosition: google.maps.LatLngLiteral | null = null;
  mapCenter: google.maps.LatLngLiteral = { lat: 0, lng: 0 };
  mapZoom = 15;
  onClose(): void {
    this.dialogRef.close();
  }
  categories: Category[] = [
    { value: 0, viewValue: 'Food and Drinks' },
    { value: 1, viewValue: 'Shopping' },
    { value: 2, viewValue: 'House' },
    { value: 3, viewValue: 'Transportation' },
    { value: 4, viewValue: 'Vehicle' },
    { value: 5, viewValue: 'Life and Entertainment' },
    { value: 6, viewValue: 'Finance' },
    { value: 7, viewValue: 'HealthAndPersonalCare' },
    { value: 8, viewValue: 'Income' },
    { value: 9, viewValue: 'Other' },
  ];
  constructor(
    public dialogRef: MatDialogRef<TransactionDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public transaction: Transaction
  ) {
    //@ts-ignore
    this.transaction.accountId = this.transaction.account;
    if (this.transaction.latitude && this.transaction.longitude) {
      this.mapCenter = {
        lat: this.transaction.latitude,
        lng: this.transaction.longitude,
      };
      this.markerPosition = this.mapCenter;
    }
  }
}
