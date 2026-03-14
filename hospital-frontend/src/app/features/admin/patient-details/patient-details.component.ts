import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AdminPatientsService } from '../admin-patients.service.ts.service';

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './patient-details.component.html',
})
export class PatientDetailsComponent implements OnInit {
  patient: any;
  appointments: any[] = [];
  medicalRecords: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private service: AdminPatientsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getPatient(id).subscribe((res) => {
      this.patient = res.patient;
      this.appointments = res.appointments;
      this.medicalRecords = res.medicalRecords;
    });
  }
}
