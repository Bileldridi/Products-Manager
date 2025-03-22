import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { NgForm } from '@angular/forms';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [FormsModule,
    InputTextModule,
    InputTextareaModule,
    ButtonModule,
    ToastModule,
    CardModule],
    providers: [MessageService],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent {
  
  constructor(private messageService: MessageService) {}

  public onSubmit(form: NgForm): void {
    if (form.valid) {
      this.messageService.add({
        severity: 'success',
        summary: 'Succès',
        detail: 'Votre message a bien été envoyé ✅'
      });

      form.resetForm();
    }
  }
}
