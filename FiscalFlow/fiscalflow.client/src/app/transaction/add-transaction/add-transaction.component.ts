import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Account} from "../../shared/models/account/account";
import {AddTransaction} from "../../shared/models/transaction/addTransaction";
import {TransactionService} from "../transaction.service";
import {Transaction} from "../../shared/models/transaction/transaction";
import {UpdateTransaction} from "../../shared/models/transaction/updateTransaction";

interface Category {
  value: number;
  viewValue: string;
}

@Component({
  selector: 'app-add-transaction',
  templateUrl: './add-transaction.component.html',
  styleUrls: ['./add-transaction.component.scss']
})
export class AddTransactionComponent implements OnInit {
  today:Date = new Date();
  categories: Category[] = [
    {value: 0, viewValue: "Food and Drinks"},
    {value: 1, viewValue: "Shopping"},
    {value: 2, viewValue: "House"},
    {value: 3, viewValue: "Transportation"},
    {value: 4, viewValue: "Vehicle"},
    {value: 5, viewValue: "Life and Entertainment"},
    {value: 6, viewValue: "Communication and Pc"},
    {value: 7, viewValue: "Financial Expenses"},
    {value: 8, viewValue: "Investments"},
    {value: 9, viewValue: "Income"},
    {value: 10, viewValue: "Other"},
  ]
  public addTransactionForm: FormGroup = new FormGroup({});

  constructor(private fb: FormBuilder,
              private transactionService: TransactionService,
              private _dialogRef: MatDialogRef<AddTransactionComponent>,
              @Inject(MAT_DIALOG_DATA) public data: {
                account: Account,
                transaction: Transaction
              }) {
    this.addTransactionForm = this.fb.group({
      description: ['', Validators.required],
      value: ['', Validators.required],
      type: [0, Validators.required],
      createdOnUtc: ['', Validators.required],
      category: ['', Validators.required],
      payee: ['', Validators.required],
      accountId: [{value: this.data.account.name, disabled: true}, Validators.required]
    })
  }

  ngOnInit(): void {
    this.addTransactionForm.patchValue(this.data.transaction);
  }

  onFormSubmit() {
    if (this.addTransactionForm.valid) {
      if(this.data.transaction != null) {
        // edit transaction
        const formValue = this.addTransactionForm.value;
        const updateTransaction: UpdateTransaction = {
          transactionId: this.data.transaction.id,
          description: formValue.description,
          payee: formValue.payee,
          type: Number(formValue.type),
          value: formValue.value,
          category: formValue.category,
          createdOnUtc: formValue.createdOnUtc,
        };
        this.transactionService.editTransaction(updateTransaction).subscribe({
          next: () => {
            this._dialogRef.close(true);
          },
          error: error => {
            this._dialogRef.close(false);
            console.log(error);
          }
        });
      } else {
        // create new transaction
        const formValue = this.addTransactionForm.value;
        let addTransactionReq: AddTransaction = {
          description: formValue.description,
          payee: formValue.payee,
          type: Number(formValue.type),
          value: formValue.value,
          category: formValue.category,
          createdOnUtc: formValue.createdOnUtc,
          accountId: this.data.account.id
        };
        this.transactionService.addTransaction(addTransactionReq).subscribe({
          next: () => {
            this._dialogRef.close(true);
          },
          error: error => {
            this._dialogRef.close(false);
            console.log(error);
          }
        });
      }
    }
  }
}
