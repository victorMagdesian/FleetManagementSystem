import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';

export interface MenuItem {
  label: string;
  icon: string;
  routerLink: string;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToastModule,
    ConfirmDialogModule,
    HeaderComponent,
    SidebarComponent
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  sidebarCollapsed = false;
  currentPageTitle = 'Dashboard';

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/dashboard'
    },
    {
      label: 'Veículos',
      icon: 'pi pi-car',
      routerLink: '/vehicles'
    },
    {
      label: 'Condutores',
      icon: 'pi pi-users',
      routerLink: '/drivers'
    },
    {
      label: 'Viagens',
      icon: 'pi pi-map',
      routerLink: '/trips'
    },
    {
      label: 'Manutenções',
      icon: 'pi pi-wrench',
      routerLink: '/maintenance'
    }
  ];

  toggleSidebar(): void {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}
