import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-doctors',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './doctors.component.html',
  styleUrl: './doctors.component.css',
})
export class DoctorsComponent implements OnInit {
  doctors: any[] = [];
  loading = true;
  adminMap: Record<number, string> = {};

  showForm = false;
  editingDoctorId: number | null = null;

  constructor(
    private service: AdminAppointmentsService,
    private fb: FormBuilder,
  ) {}

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', Validators.required],
    specialization: ['', Validators.required],
  });

  ngOnInit() {
    this.service.getAdmins().subscribe((admins) => {
      admins.forEach((a: any) => {
        this.adminMap[a.id] = `${a.firstName} ${a.lastName}`;
      });
    });
    this.loadDoctors();
  }

  loadDoctors() {
    this.service.getDoctors().subscribe((res) => {
      this.doctors = res;
      this.loading = false;
    });
  }

  getAdminName(id: number | null): string {
    if (!id) return '—';
    return this.adminMap[id] ?? `ID ${id}`;
  }

  openCreate() {
    this.showForm = true;
    this.editingDoctorId = null;
    this.form.reset();
  }

  editDoctor(d: any) {
    this.showForm = true;
    this.editingDoctorId = d.id;
    this.form.patchValue({
      firstName: d.firstName,
      lastName: d.lastName,
      email: d.email,
      specialization: d.specialization,
    });
  }

  saveDoctor() {
    if (this.form.invalid) return;

    const payload = {
      firstName: this.form.value.firstName ?? '',
      lastName: this.form.value.lastName ?? '',
      email: this.form.value.email ?? '',
      specialization: this.form.value.specialization ?? '',
    };

    if (this.editingDoctorId) {
      this.service.updateDoctor(this.editingDoctorId, payload).subscribe(() => {
        this.showForm = false;
        this.loadDoctors();
      });
    } else {
      this.service.createDoctor(payload).subscribe(() => {
        this.showForm = false;
        this.loadDoctors();
      });
    }
  }

  doctorToDelete: { id: number; name: string } | null = null;

  confirmDelete(id: number, name: string) {
    this.doctorToDelete = { id, name };
  }

  cancelDelete() {
    this.doctorToDelete = null;
  }

  deleteDoctor() {
    if (!this.doctorToDelete) return;
    this.service.deleteDoctor(this.doctorToDelete.id).subscribe(() => {
      this.doctorToDelete = null;
      this.loadDoctors();
    });
  }
}
