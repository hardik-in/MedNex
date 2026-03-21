import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorTimeslotsComponent } from './doctor-timeslots.component';

describe('DoctorTimeslotsComponent', () => {
  let component: DoctorTimeslotsComponent;
  let fixture: ComponentFixture<DoctorTimeslotsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorTimeslotsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DoctorTimeslotsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
