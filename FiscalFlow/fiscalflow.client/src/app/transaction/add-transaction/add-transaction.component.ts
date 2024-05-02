import {Component, Inject, Input, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {Account} from "../../shared/models/account/account";
import {AddTransaction} from "../../shared/models/transaction/addTransaction";
import {TransactionService} from "../transaction.service";
import {error} from "@angular/compiler-cli/src/transformers/util";

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
  dataAcc:boolean = true;
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
              @Inject(MAT_DIALOG_DATA) public data: Account) {
    this.addTransactionForm = this.fb.group({
      description: ['', Validators.required],
      value:[0, Validators.required],
      transactionType:[0, Validators.required],
      transactionDate:['', Validators.required],
      category:['', Validators.required],
      payee: ['', Validators.required],
      accountId: [{value:this.data.name, disabled: true}, Validators.required]
    })
    console.log(this.dataAcc);
  }

  ngOnInit(): void {
   // this.addTransactionForm.patchValue(this.data);
  }

  onFormSubmit() {
    if(this.addTransactionForm.valid) {
        // make rest req
        const formValue = this.addTransactionForm.value;
        console.log(formValue);
        let addTransactionReq:AddTransaction = {
          description: formValue.description,
          payee:formValue.payee,
          type: Number(formValue.transactionType),
          value: formValue.value,
          category: formValue.category,
          createdOnUtc: formValue.transactionDate,
          accountId: this.data.id
        };
        this.transactionService.addTransaction(addTransactionReq).subscribe({
          next: data => {
            this._dialogRef.close(true);
          },
          error: error => {
            this._dialogRef.close(false);
            console.log(error);
          }
        })
    }
  }
}
