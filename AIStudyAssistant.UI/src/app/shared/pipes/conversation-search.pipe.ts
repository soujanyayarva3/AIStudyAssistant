import { Pipe, PipeTransform } from '@angular/core';

import { Conversation } from '../../core/models/conversation';
@Pipe({
  name: 'conversationSearch',
  standalone: true
})
export class ConversationSearchPipe implements PipeTransform {

  transform(
    conversations: Conversation[],
    search: string
  ): Conversation[] {

    if (!search)
      return conversations;

    return conversations.filter(x =>
      x.title.toLowerCase()
      .includes(search.toLowerCase())
    );

  }

}