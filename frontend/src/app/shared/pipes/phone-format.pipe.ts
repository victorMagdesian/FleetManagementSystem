import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'phoneFormat',
  standalone: true
})
export class PhoneFormatPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    
    try {
      // Remove all non-digit characters
      const cleaned = value.replace(/\D/g, '');
      
      // Format: +5511999999999 -> +55 (11) 99999-9999
      if (cleaned.length === 13) {
        return `+${cleaned.slice(0, 2)} (${cleaned.slice(2, 4)}) ${cleaned.slice(4, 9)}-${cleaned.slice(9)}`;
      }
      
      // Format: 11999999999 -> (11) 99999-9999
      if (cleaned.length === 11) {
        return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 7)}-${cleaned.slice(7)}`;
      }
      
      // Format: 1999999999 -> (19) 9999-9999
      if (cleaned.length === 10) {
        return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 6)}-${cleaned.slice(6)}`;
      }
      
      // Return original value if format doesn't match
      return value;
    } catch (error) {
      console.error('Error formatting phone:', error);
      return value;
    }
  }
}
