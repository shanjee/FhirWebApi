import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-create-temperature',
  templateUrl: './create-temperature.component.html',
  styleUrls: ['./create-temperature.component.css']
})
export class CreateTemperatureComponent implements OnInit {

  temperature = new FormControl('');

  constructor() { }

  ngOnInit() {
  }

  SaveNewReading() {
    this.temperature.setValue('');
  }

}
