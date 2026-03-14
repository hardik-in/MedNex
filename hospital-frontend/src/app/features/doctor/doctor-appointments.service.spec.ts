import { TestBed } from '@angular/core/testing';

import { DoctorAppointmentsService } from './doctor-appointments.service';

describe('DoctorAppointmentsService', () => {
  let service: DoctorAppointmentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DoctorAppointmentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
