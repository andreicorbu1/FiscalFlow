import {Component, Input, OnInit} from '@angular/core';
import {Category} from "../../shared/models/transaction/enums/category";
import {AccountService} from "../account.service";

export interface Expense {
  category: string,
  value: number
}

@Component({
  selector: 'app-category-spending',
  templateUrl: './category-spending.component.html',
  styleUrls: ['./category-spending.component.scss']
})
export class CategorySpendingComponent implements OnInit{
  categories: any;
  constructor(private accountService: AccountService) {
    }
    ngOnInit(): void {
      this.accountService.getCategoryExpenses().subscribe(data => {
        this.categories = data;
      })
    }
    getCategoryClass(category: any): string {
      switch (category) {
        case 'FoodAndDrinks':
          return 'food-and-drinks';
        case 'Shopping':
          return 'shopping';
        case 'LifeAndEntertainment':
          return 'Life-And-Entertainment';
        default:
          return '';
      }
    }
}
