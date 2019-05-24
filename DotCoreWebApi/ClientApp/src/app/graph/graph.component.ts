import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { FormBuilder, Validators } from '@angular/forms';
import { TemperatureModel } from '../model/temperature-model';

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.css']
})
export class GraphComponent implements OnInit {

  public graphCollection: GraphDataCollection;
  public chartLegend: boolean = true;
  public chartType: string = 'line';

  temperature = new FormControl('');
  temperatureForm: any;


  ngOnInit() {
    this.temperatureForm = this.formbulider.group({
      StartDate: ['', [Validators.required]],
      EndDate: ['', [Validators.required]],
      Temperature: ['', [Validators.required]]
    });
  }

  onFormSubmit() {

    const temperatureData = this.temperatureForm.value;
    this.filterTemperature(temperatureData);    
  }


  filterTemperature(temperature: TemperatureModel) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

    var queryParameter = 'startdate='+temperature.StartDate+'&enddate='+temperature.EndDate;

    this.http.get<GraphDataCollection>(this.baseUrl + 'api/TemperatureGraph/GetBodyTemperatureAql?'+queryParameter).subscribe(result => {
      this.graphCollection = result;
    }, error => console.error(error));
  }



  public chartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom'
    }
  };

  constructor(private formbulider: FormBuilder, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
    //http.get<GraphDataCollection>(baseUrl + 'api/TemperatureGraph/GetBodyTemperatureAql').subscribe(result => {
    //  this.graphCollection = result;
    //}, error => console.error(error));
  }
}


interface GraphData {
  data: Array<number>;
  label: string;
} 

interface GraphDataCollection {
  weatherList: GraphData[];
  chartLabels: string[];
}  
