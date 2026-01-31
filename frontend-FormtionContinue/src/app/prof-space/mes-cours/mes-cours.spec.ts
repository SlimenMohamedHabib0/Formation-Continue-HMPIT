import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MesCours } from './mes-cours';

describe('MesCours', () => {
  let component: MesCours;
  let fixture: ComponentFixture<MesCours>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MesCours]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MesCours);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
