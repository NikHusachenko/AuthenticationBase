import { Component, Input } from '@angular/core';
import { MyTableRowComponent } from "../my-table-row/my-table-row.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-my-table',
  standalone: true,
  imports: [MyTableRowComponent, CommonModule],
  templateUrl: './my-table.component.html',
  styleUrl: './my-table.component.css'
})
export class MyTableComponent {
  @Input() tableData: string[] = [];
}