import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MesResultats } from './mes-resultats';

describe('MesResultats', () => {
  let component: MesResultats;
  let fixture: ComponentFixture<MesResultats>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MesResultats]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MesResultats);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
