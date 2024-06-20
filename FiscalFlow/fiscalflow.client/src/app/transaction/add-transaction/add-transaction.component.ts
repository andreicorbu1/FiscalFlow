import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Account } from '../../shared/models/account/account';
import { AddTransaction } from '../../shared/models/transaction/addTransaction';
import { TransactionService } from '../transaction.service';
import { Transaction } from '../../shared/models/transaction/transaction';
import { UpdateTransaction } from '../../shared/models/transaction/updateTransaction';
import { MAT_RADIO_DEFAULT_OPTIONS } from '@angular/material/radio';
import { SharedService } from '../../shared/shared.service';
import { MAT_SLIDE_TOGGLE_DEFAULT_OPTIONS } from '@angular/material/slide-toggle';
import { OpenaiService } from 'src/app/openai.service';

interface Category {
  value: number;
  viewValue: string;
}

@Component({
  selector: 'app-add-transaction',
  templateUrl: './add-transaction.component.html',
  providers: [
    {
      provide: MAT_RADIO_DEFAULT_OPTIONS,
      useValue: { color: 'primary' },
    },
    {
      provide: MAT_SLIDE_TOGGLE_DEFAULT_OPTIONS,
      useValue: { color: 'primary' },
    },
  ],
  styleUrls: ['./add-transaction.component.scss'],
})
export class AddTransactionComponent implements OnInit {
  markerPosition: google.maps.LatLngLiteral | null = null;
  mapCenter: google.maps.LatLngLiteral = { lat: 0, lng: 0 };
  mapZoom = 15;
  suggestedCategory: string | null = null;
  today: Date = new Date();
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
  public addTransactionForm: FormGroup = new FormGroup({});

  constructor(
    private fb: FormBuilder,
    private transactionService: TransactionService,
    private openaiService: OpenaiService,
    private sharedService: SharedService,
    private _dialogRef: MatDialogRef<AddTransactionComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      account: Account;
      transaction: Transaction;
    }
  ) {
    this.addTransactionForm = this.fb.group({
      accountId: [this.data.account.name],
      description: ['', Validators.required],
      value: ['', Validators.required],
      type: ['', Validators.required],
      createdOnUtc: ['', Validators.required],
      category: ['', Validators.required],
      payee: ['', Validators.required],
      enableLocation: [false],
      location: [''],
      isRecursive: [false],
      recurrencePeriod: [{ value: '', disabled: true }],
    });
    this.addTransactionForm
      .get('isRecursive')!
      .valueChanges.subscribe((value) => {
        if (value) {
          this.addTransactionForm.get('recurrencePeriod')!.enable();
          this.addTransactionForm
            .get('recurrencePeriod')!
            .setValidators([Validators.required, Validators.min(1)]);
        } else {
          this.addTransactionForm.get('recurrencePeriod')!.disable();
          this.addTransactionForm.get('recurrencePeriod')!.clearValidators();
        }
        this.addTransactionForm
          .get('recurrencePeriod')!
          .updateValueAndValidity();
      });
  }

  onMapClick(event: google.maps.MapMouseEvent) {
    this.markerPosition = event.latLng!.toJSON();
    this.getAddressFromLatLng(this.markerPosition.lat, this.markerPosition.lng);
  }

  ngOnInit(): void {
    if (navigator.geolocation && this.data.transaction == null) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.mapCenter = {
          lat: position.coords.latitude,
          lng: position.coords.longitude,
        };
        this.markerPosition = this.mapCenter;
        this.getAddressFromLatLng(this.mapCenter.lat, this.mapCenter.lng);
      });
    }
    if (this.data.transaction != null) {
      if (this.data.transaction.reccurencePeriod != null) {
        this.addTransactionForm.patchValue({
          isRecursive: true,
        });
        this.addTransactionForm.patchValue({
          recurrencePeriod: this.data.transaction.reccurencePeriod,
        });
      }
      if (
        this.data.transaction.latitude != null &&
        this.data.transaction.longitude != null
      ) {
        this.addTransactionForm.patchValue({
          enableLocation: true,
        });
        this.markerPosition = {
          lat: this.data.transaction.latitude,
          lng: this.data.transaction.longitude,
        };
        this.mapCenter = this.markerPosition;
        this.getAddressFromLatLng(
          this.markerPosition.lat,
          this.markerPosition.lng
        );
      }
      this.addTransactionForm.patchValue(this.data.transaction);
      this.addTransactionForm.patchValue({
        type: this.data.transaction.type == 0 ? '0' : '1',
      });
    }
  }

  onPayeeAndDescriptionChange($event: Event | boolean) {
    const payee = this.addTransactionForm.get('payee')!.value;
    const description = this.addTransactionForm.get('description')!.value;
    const type = this.addTransactionForm.get('type')!.value;
    if (payee && description && $event === true) {
      this.openaiService.getSuggestedCategories(payee, description).subscribe({
        next: (response) => {
          //@ts-ignore
          this.suggestedCategory = response.choices[0].message.content;
        },
        error: (error) => {
          console.log(error);
        },
      });
    }
  }

  getAddressFromLatLng(lat: number, lng: number) {
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode({ location: { lat, lng } }, (results, status) => {
      if (status === 'OK' && results![0]) {
        this.addTransactionForm
          .get('location')!
          .setValue(results![0].formatted_address);
      } else {
        console.error('Geocoder failed due to: ' + status);
      }
    });
  }

  onFormSubmit() {
    if (this.addTransactionForm.valid) {
      if (this.data.transaction != null) {
        // edit transaction
        const formValue = this.addTransactionForm.value;
        let reccurencePeriod: number | null = null;
        if (
          this.data.transaction.reccurencePeriod != null &&
          formValue.isRecursive === false
        ) {
          reccurencePeriod = 0;
        } else if (
          this.data.transaction.reccurencePeriod !== formValue.recurrencePeriod
        ) {
          reccurencePeriod = formValue.recurrencePeriod;
        }
        const updateTransaction: UpdateTransaction = {
          transactionId: this.data.transaction.id,
          description: formValue.description,
          payee: formValue.payee,
          imageUrl: null,
          longitude: this.markerPosition ? this.markerPosition.lng : null,
          latitude: this.markerPosition ? this.markerPosition.lat : null,
          recurrence: reccurencePeriod,
          type: Number(formValue.type),
          value: formValue.value,
          category: formValue.category,
          createdOnUtc: formValue.createdOnUtc,
        };
        this.transactionService.editTransaction(updateTransaction).subscribe({
          next: () => {
            this._dialogRef.close(true);
          },
          error: (error) => {
            this._dialogRef.close(false);
            const errorMessage: string = error.error.detail;
            const displayMessage: string = errorMessage
              .substring(errorMessage.indexOf('*') + 1)
              .trim();
            this.sharedService.showNotification(
              false,
              'Edit transaction',
              displayMessage
            );
            console.log(error);
            console.log(error);
          },
        });
      } else {
        // create new transaction
        const formValue = this.addTransactionForm.value;
        let addTransactionReq: AddTransaction = {
          description: formValue.description,
          payee: formValue.payee,
          type: Number(formValue.type),
          value: formValue.value,
          imageUrl: null,
          isRecursive: formValue.isRecursive,
          recurrence: formValue.recurrencePeriod,
          category: formValue.category,
          longitude: this.markerPosition ? this.markerPosition.lng : null,
          latitude: this.markerPosition ? this.markerPosition.lat : null,
          createdOnUtc: formValue.createdOnUtc,
          accountId: this.data.account.id,
        };
        if (formValue.enableLocation === false) {
          addTransactionReq.longitude = null;
          addTransactionReq.latitude = null;
        }
        this.transactionService.addTransaction(addTransactionReq).subscribe({
          next: () => {
            this._dialogRef.close(true);
          },
          error: (error) => {
            this._dialogRef.close(false);
            const errorMessage: string = error.error.detail;
            const displayMessage: string = errorMessage
              .substring(errorMessage.indexOf('*') + 1)
              .trim();
            this.sharedService.showNotification(
              false,
              'Add Transaction',
              displayMessage
            );
            console.log(error);
          },
        });
      }
    }
  }
}
