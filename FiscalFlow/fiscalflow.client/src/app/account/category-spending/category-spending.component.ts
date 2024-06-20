import { Component, Input, OnInit } from '@angular/core';
import { Category } from '../../shared/models/transaction/enums/category';
import { AccountService } from '../account.service';
import { ChartData } from 'chart.js';

export interface Expense {
  category: string;
  value: number;
}

@Component({
  selector: 'app-category-spending',
  templateUrl: './category-spending.component.html',
  styleUrls: ['./category-spending.component.scss'],
})
export class CategorySpendingComponent implements OnInit {
  categories: any;
  expensesIncomeData: ChartData<'doughnut', number[], string>;
  categoryExpensesData: ChartData<'pie', number[], string>;
  expensesIncomeLabels: string[] = ['Expenses', 'Income'];
  categoryExpensesLabels: string[];
  showExpenseChart: boolean = false;
  showIncomeChart: boolean = false;
  constructor(private accountService: AccountService) {}
  ngOnInit(): void {
    this.accountService.getCategoryExpenses().subscribe((data) => {
      this.categories = data;
      const totalExpenses = Object.entries(this.categories)
        .filter(([key]) => key !== 'Income')
        .reduce((sum, [_, value]) => sum + (value as number), 0);

      this.expensesIncomeData = {
        labels: this.expensesIncomeLabels,
        datasets: [
          {
            data: [totalExpenses, this.categories['Income'] as number],
            backgroundColor: ['#FF6384', '#36A2EB'],
            hoverBackgroundColor: ['#FF6384', '#36A2EB'],
          },
        ],
      };

      const categoryExpensesDataArray = Object.entries(this.categories)
        .filter(([key, value]) => key !== 'Income')
        .map(([_, value]) => value as number);
      this.showExpenseChart = categoryExpensesDataArray.length > 0;
      this.categoryExpensesLabels = Object.keys(this.categories).filter(
        (key) => key !== 'Income'
      );

      this.categoryExpensesData = {
        labels: this.categoryExpensesLabels,
        datasets: [
          {
            data: categoryExpensesDataArray,
            backgroundColor: this.categoryExpensesLabels.map(() =>
              this.getRandomColor()
            ),
            hoverBackgroundColor: this.categoryExpensesLabels.map(() =>
              this.getRandomColor()
            ),
          },
        ],
      };
    });
  }
  getCategoryClass(category: any): string {
    switch (category) {
      case 'FoodAndDrinks':
        return 'Food-and-Drinks';
      case 'LifeAndEntertainment':
        return 'Life-and-Entertainment';
      case 'HealthAndPersonalCare':
        return 'Health-and-Personal-Care';
      default:
        return category.toLowerCase();
    }
  }

  getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
}
