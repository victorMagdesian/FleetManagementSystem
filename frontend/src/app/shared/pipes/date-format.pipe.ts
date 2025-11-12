import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@angular/common';

@Pipe({
  name: 'dateFormat',
  standalone: true
})
export class DateFormatPipe implements PipeTransform {
  transform(value: Date | string | null | undefined, format: string = 'dd/MM/yyyy HH:mm'): string {
    if (!value) return '';
    
    try {
      const date = typeof value === 'string' ? new Date(value) : value;
      return formatDate(date, format, 'pt-BR');
    } catch (error) {
      console.error('Error formatting date:', error);
      return '';
    }
  }
}
