import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';


@Component({
  selector: 'app-prof-layout',
  standalone: true,
  imports: [Navbar, RouterOutlet],
  templateUrl: './prof-layout.html',
  styleUrl: './prof-layout.css',
})
export class ProfLayout {}
