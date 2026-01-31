import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LectureCours } from './lecture-cours';

describe('LectureCours', () => {
  let component: LectureCours;
  let fixture: ComponentFixture<LectureCours>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LectureCours]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LectureCours);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
