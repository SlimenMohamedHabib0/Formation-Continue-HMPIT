import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Supervision } from './supervision';

describe('Supervision', () => {
  let component: Supervision;
  let fixture: ComponentFixture<Supervision>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Supervision]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Supervision);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
