import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-icon',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './icon.component.html',
  styleUrls: ['./icon.component.scss']
})
export class IconComponent {
  @Input() name!: string; // PrimeIcon name (e.g., 'pi-home', 'pi-user')
  @Input() size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' = 'md';
  @Input() color?: 'primary' | 'success' | 'warning' | 'danger' | 'secondary' | 'muted' | 'white';
  @Input() spin = false;
  @Input() ariaLabel?: string;

  get iconClasses(): string {
    const classes = ['app-icon'];
    
    // Add PrimeIcon class
    if (this.name) {
      classes.push('pi', this.name.startsWith('pi-') ? this.name : `pi-${this.name}`);
    }
    
    // Add size class
    classes.push(`app-icon--${this.size}`);
    
    // Add color class
    if (this.color) {
      classes.push(`app-icon--${this.color}`);
    }
    
    // Add spin class
    if (this.spin) {
      classes.push('pi-spin');
    }
    
    return classes.join(' ');
  }
}
