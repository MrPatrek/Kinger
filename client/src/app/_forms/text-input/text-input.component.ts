import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})

  export class TextInputComponent implements ControlValueAccessor {
    @Input() label = '';
    @Input() type = 'text';     // we can later overwrite this (e.g. for password case)

  constructor(@Self() public ngControl: NgControl) {        // @Self() is here because we don't want to REuse any other NgControl from the memory
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }


}
