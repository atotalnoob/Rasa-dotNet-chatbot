from __future__ import absolute_import
from __future__ import division
from __future__ import print_function
from __future__ import unicode_literals

from typing import Any
from typing import Dict
from typing import List
from typing import Text

from rasa_nlu.components import Component


class EntityExtractor(Component):
    def add_extractor_name(self, entities):
        # type: (List[Dict[Text, Any]]) -> List[Dict[Text, Any]]
        for entity in entities:
            entity["extractor"] = self.name
        return entities

    def add_suggestionor_name(self, entity):
        # type: (Dict[Text, Any]) -> Dict[Text, Any]
        if "suggestionors" in entity:
            entity["suggestionors"].append(self.name)
        else:
            entity["suggestionors"] = [self.name]
        return entity
