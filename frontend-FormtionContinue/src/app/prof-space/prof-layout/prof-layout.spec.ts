import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfLayout } from './prof-layout';

describe('ProfLayout', () => {
  let component: ProfLayout;
  let fixture: ComponentFixture<ProfLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
