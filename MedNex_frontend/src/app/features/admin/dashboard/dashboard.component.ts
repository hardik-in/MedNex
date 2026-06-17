import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AdminService } from '../admin.service';

interface DashboardStats {
  totalDoctors:      number;
  totalPatients:     number;
  totalAppointments: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class AdminDashboardComponent implements OnInit {
  private service = inject(AdminService);

  stats: DashboardStats = { totalDoctors: 0, totalPatients: 0, totalAppointments: 0 };
  loading = true;
  error   = '';

  ngOnInit(): void {
    // Single endpoint built for this — avoids three parallel calls just for counts
    this.service.getDashboardStats().subscribe({
      next: (res) => {
        this.stats   = res as DashboardStats;
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load dashboard stats.';
        this.loading = false;
      },
    });
  }
}   