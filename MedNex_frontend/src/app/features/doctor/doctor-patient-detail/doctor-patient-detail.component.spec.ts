import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorPatientDetailComponent } from './doctor-patient-detail.component';

describe('DoctorPatientDetailComponent', () => {
  let component: DoctorPatientDetailComponent;
  let fixture: ComponentFixture<DoctorPatientDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorPatientDetailComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DoctorPatientDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
