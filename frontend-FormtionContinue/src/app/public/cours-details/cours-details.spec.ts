import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursDetails } from './cours-details';

describe('CoursDetails', () => {
  let component: CoursDetails;
  let fixture: ComponentFixture<CoursDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoursDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
