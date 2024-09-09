import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-my-table-row',
  standalone: true,
  imports: [],
  templateUrl: './my-table-row.component.html',
  styleUrl: './my-table-row.component.css'
})
export class MyTableRowComponent {
  @Input() data: string | undefined;
}
