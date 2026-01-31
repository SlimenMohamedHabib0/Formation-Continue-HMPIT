import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Inscriptions } from './inscriptions';

describe('Inscriptions', () => {
  let component: Inscriptions;
  let fixture: ComponentFixture<Inscriptions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Inscriptions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Inscriptions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
