import { Component, Input, Output, EventEmitter, ContentChild, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { SearchBoxComponent } from '../../molecules/search-box/search-box.component';
import { ButtonComponent } from '../../atoms/button/button.component';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    SearchBoxComponent,
    ButtonComponent
  ],
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent {
  @Input() data: any[] = [];
  @Input() loading = false;
  @Input() searchPlaceholder = 'Buscar...';
  @Input() addButtonLabel = 'Adicionar';
  @Input() filterFields: string[] = [];
  @Input() showAddButton = true;
  @Input() showSearchBox = true;
  @Input() rows = 10;
  @Input() paginator = true;
  
  @Output() add = new EventEmitter<void>();
  @Output() searchChange = new EventEmitter<string>();
  
  @ContentChild('header') headerTemplate?: TemplateRef<any>;
  @ContentChild('body') bodyTemplate?: TemplateRef<any>;
  
  globalFilterValue = '';
  
  onSearch(searchTerm: string): void {
    this.globalFilterValue = searchTerm;
    this.searchChange.emit(searchTerm);
  }
  
  onAdd(): void {
    this.add.emit();
  }
}
