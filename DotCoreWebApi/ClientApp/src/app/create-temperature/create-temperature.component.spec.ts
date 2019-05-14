import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTemperatureComponent } from './create-temperature.component';

describe('CreateTemperatureComponent', () => {
  let component: CreateTemperatureComponent;
  let fixture: ComponentFixture<CreateTemperatureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateTemperatureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateTemperatureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
