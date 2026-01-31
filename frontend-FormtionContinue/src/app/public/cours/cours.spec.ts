import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Cours } from './cours';

describe('Cours', () => {
  let component: Cours;
  let fixture: ComponentFixture<Cours>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Cours]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Cours);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
