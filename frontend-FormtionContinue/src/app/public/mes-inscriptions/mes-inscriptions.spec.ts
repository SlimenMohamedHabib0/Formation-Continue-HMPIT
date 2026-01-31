import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MesInscriptions } from './mes-inscriptions';

describe('MesInscriptions', () => {
  let component: MesInscriptions;
  let fixture: ComponentFixture<MesInscriptions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MesInscriptions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MesInscriptions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
