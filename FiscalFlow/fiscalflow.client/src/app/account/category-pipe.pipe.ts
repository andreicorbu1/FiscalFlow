import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'categoryPipe',
})
export class CategoryPipePipe implements PipeTransform {
  transform(value: string, ...args: unknown[]): string {
    value = value.replaceAll('-', ' ');
    value = value.charAt(0).toUpperCase() + value.slice(1);
    return value;
  }
}
