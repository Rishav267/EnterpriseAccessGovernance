import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskFindingsComponent } from './risk-findings.component';

describe('RiskFindingsComponent', () => {
  let component: RiskFindingsComponent;
  let fixture: ComponentFixture<RiskFindingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskFindingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RiskFindingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
